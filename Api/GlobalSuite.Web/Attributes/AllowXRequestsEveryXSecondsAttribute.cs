using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Controllers;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

// https://github.com/johnstaveley/SecurityEssentials/blob/master/SecurityEssentials/Core/Attributes/AllowXRequestsEveryXSeconds.cs
namespace GlobalSuite.Web.Attributes
{
    /// <summary>
    /// SECURE: Decorates any route that needs to have client requests limited over time.
    /// </summary>
    /// <remarks>
    /// Uses the current System.Web.Caching.Cache to store each client request to the decorated route.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowXRequestsEveryXSecondsAttribute : ActionFilterAttribute
        {
		/// <summary>
		/// A unique name for this Throttle.
		/// </summary>
		/// <remarks>
		/// We'll be inserting a Cache record based on this name and client IP, e.g. "Name-192.168.0.1"
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// The number of seconds clients must wait before executing this decorated route again.
		/// </summary>
		public int Seconds { get; set; }

		/// <summary>
		/// The number of requests to allow per client in the given number of seconds
		/// </summary>
		public int Requests { get; set; }

		/// <summary>
		/// A text message (not themed) that will be sent to the client upon throttling.  You can include the token {n} to
		/// show this.Seconds in the message, e.g. "You have performed this action more than {x} times in the last {n} seconds.".
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The content name (themed and from SiteContent) to show upon throttling.  If this is present, the Message parameter will not be used.
		/// </summary>
		public string ContentName { get; set; }

		// Used to get around weird cache behavior with value types
		private class Int32Value
		{
			public Int32Value()
			{
				Value = 1;
			}
			public int Value { get; set; }
		}

		 

		public override void OnActionExecuting(HttpActionContext c)
		{
			if (c == null) throw new ArgumentException("ActionExecutingContext not spcecified");
			var key = string.Concat("AllowXRequestsEveryXSeconds-", Name, "-", HttpContext.Current.Request.UserHostAddress);
			var allowExecute = false;

			var currentCacheValue = HttpRuntime.Cache[key];
			if (currentCacheValue == null)
			{
				HttpRuntime.Cache.Add(key,
					new Int32Value(),
					null, // no dependencies
					DateTime.Now.AddSeconds(Seconds), // absolute expiration
					Cache.NoSlidingExpiration,
					CacheItemPriority.Low,
					null); // no callback

				allowExecute = true;
			}
			else
			{
				var value = (Int32Value)currentCacheValue;
				value.Value++;
				if (value.Value <= Requests)
				{
					allowExecute = true;
				}
			}

			if (allowExecute) return;
			if (string.IsNullOrEmpty(Message))
				Message = "You have performed this action more than {x} times in the last {n} seconds.";

			// if (!string.IsNullOrEmpty(ContentName))
			// {
			// 	//use SiteContent
			// 	c. = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "WebPageContent" },
			// 		{ "Action", ContentName } });
			// }
			else
			{
				var formattedMessage = Message.Replace("{x}",
					Requests.ToString(CultureInfo.CurrentCulture)).Replace("{n}",
					Seconds.ToString(CultureInfo.CurrentCulture));
				//just send a message (not themed)
				var res = new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.Conflict,
					ReasonPhrase = formattedMessage,
					Content =new StringContent(formattedMessage)
				};
				// res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				c.Response = res;
			}
			// // see 409 - http://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
			HttpContext.Current.Response.TrySkipIisCustomErrors = true; //to prevent iis from showing default 409 page
			HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Conflict;
		}
	}

}