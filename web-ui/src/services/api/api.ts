import {ApiConfig, DEFAULT_API_CONFIG} from './api-config';
import axios, {AxiosInstance} from 'axios';

export class Api {
  /**
   * The underlying  instance which performs the requests.
   */
  apiService: AxiosInstance | undefined;

  /**
   * Configurable options.
   */
  config: ApiConfig;

  /**
   * Creates the api.
   *
   * @param config The configuration to use.
   */
  constructor(config: ApiConfig = DEFAULT_API_CONFIG) {
    this.config = config;
  }

  /**
   * Sets up the API.  This will be called during the bootup
   * sequence and will happen before the first React component
   * is mounted.
   *
   * Be as quick as possible in here.
   */

  setup() {
    // construct the apisauce instance

    this.apiService = axios.create({
      baseURL: this.config.url,
      timeout: this.config.timeout,
      headers: {
        Accept: 'application/json',
      },
    });

    // Add a response interceptor
    this.apiService?.interceptors.response.use(
      response => {
        // Any status code that lie within the range of 2xx cause this function to trigger
        // Do something with response data
        return response;
      },
      error => {
        // Any status codes that falls outside the range of 2xx cause this function to trigger
        // Do something with response error
        console.log(error);
        return Promise.reject(error);
      }
    );
  }
}
