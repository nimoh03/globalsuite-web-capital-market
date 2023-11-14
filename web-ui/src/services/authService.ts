import { httpService } from "./httpService";

class AuthService {
  async login(payload: any) {
    const response = await httpService.post("/api/token", payload);

    return response;
  }
}

export const authService = new AuthService();
