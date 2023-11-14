import { toast } from "react-toastify";

class NotificationService {
  static async show(
    message = "",
    type = toast.TYPE.DEFAULT,
    position = toast.POSITION.TOP_RIGHT,
    autoClose = 5000
  ) {
    const defaultOptions = {
      position: position,
      autoClose: autoClose,
      toastId: "mh-test",
    };

    switch (type) {
      case "default":
        toast.dark(message, defaultOptions);
        break;
      case "error":
        toast.error(message, defaultOptions);
        break;
      case "info":
        toast.info(message, defaultOptions);
        break;
      case "success":
        toast.success(message, defaultOptions);
        break;
      case "warning":
        toast.warn(message, defaultOptions);
        break;
      default:
        toast(message, defaultOptions);
    }
  }
}

export default NotificationService;
