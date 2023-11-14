import { accountingService } from "@/services/accountingService";
import { makeAutoObservable } from "mobx";

class AccountingPaymentsStore {
  constructor() {
    makeAutoObservable(this);
  }

  async getPayments(payload?: any) {
    const response = await accountingService.getPayments(payload);

    console.log(response);
  }
}

export default AccountingPaymentsStore;
