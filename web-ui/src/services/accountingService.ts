import { httpService } from "./httpService";

class AccountingService {
  async getPayments(payload?: any) {
    const response = await httpService.get("/api/accounting/payments", payload);
    return response;
  }
}

export const accountingService = new AccountingService();
