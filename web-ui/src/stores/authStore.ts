import { AUTH_TOKEN } from "@/lib/constants";
import NotificationService from "@/services/NotificationService";
import { authService } from "@/services/authService";
import { makeAutoObservable } from "mobx";

class AuthStore {
  constructor() {
    makeAutoObservable(this);
  }

  get isAuthenticated() {
    return !!localStorage.getItem(AUTH_TOKEN);
  }

  async login(payload: any) {
    const response = await authService.login(payload);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage ||
          "invalid username/password",
        "error"
      );
    } else {
      localStorage.setItem(AUTH_TOKEN, response.data?.result?.token);
    }

    return response;
  }

  logout() {
    window.localStorage.clear();
    window.location.href = "/";
  }
}

export default AuthStore;
