import { httpService } from "./httpService";
class depositServices {
  async getAllDeposit(status) {
    const res = await httpService.get(
      `/api/accounting/deposits?status=${status}`
    );

    return res;
  }
  async getDepositByCode(code) {
    const res = await httpService.get(`/api/accounting/deposits/${code}`);

    return res;
  }
  async getPaymentByCode(code) {
    const res = await httpService.get(`/api/accounting/payments/${code}`);

    return res;
  }
  // get a credit note 
  async getCreditNoteByCode(code,status) {
    const res = await httpService.get(`/api/accounting/credit-notes/${code}?status=${status}`);

    return res;
  }
  
    // get a Debit note 
    async getDebitNoteByCode(code,status) {
      const res = await httpService.get(`/api/accounting/debit-notes/${code}?status=${status}`);
  
      return res;
    }
      // get a OpeningBalance note 
      async getOpeningBalanceByCode(code,status= 'Posted') {
        console.log(status)
        const res = await httpService.get(`/api/accounting/opening-balances/${code}${status === 'UnPosted'? '': `?status=${status}`}`);
    
        return res;
      }

      async getCustomerTransferByCode(code,status= 'Posted') {
        console.log(status)
        const res = await httpService.get(`/api/accounting/transfers/${code}${status === 'UnPosted'? '': `?status=${status}`}`);
    
        return res;
      }
  // approveTransaction
  async approveTransaction(code) {
    const res = await httpService.post(`/api/accounting/deposits/${code}`);
    return res;
  }
  async approvePaymentTransaction(code) {
    const res = await httpService.post(`/api/accounting/payments/${code}`);
    return res;
  }
  async approveCreditNoteTransaction(code) {
    const res = await httpService.post(`/api/accounting/credit-notes/post/${code}`);
    return res;
  }
  async approveDebitNoteTransaction(code) {
    const res = await httpService.post(`/api/accounting/debit-notes/post/${code}`);
    return res;
  }
  async approveCustomerOpeningBal(code) {
    const res = await httpService.post(`/api/accounting/opening-balances/post/${code}`);
    return res;
  }
  async approveCustomerTransfer(code) {
    const res = await httpService.post(`/api/accounting/transfers/post/${code}`);
    return res;
  }

  // update deposited transaction 
  async updateCustomerTransaction(payload) {
    const res = await httpService.post("/api/accounting/deposits/edit", payload);

    return res;
  }
  async updatePaymentTransaction(payload) {
    const res = await httpService.post("/api/accounting/payments/edit", payload);

    return res;
  }
  async updateCreditNoteTransaction(payload) {
    const res = await httpService.post("/api/accounting/credit-notes/edit", payload);

    return res;
  }

  async updateDebitNoteTransaction(payload) {
    const res = await httpService.post("/api/accounting/debit-notes/edit", payload);

    return res;
  }

  async updateCustomerOpeningBalTransaction(payload) {
    const res = await httpService.post("/api/accounting/opening-balances/edit", payload);

    return res;
  }


  //reverse transaction
  async reverseTransaction(code) {
    const res = await httpService.post(
      `/api/accounting/deposits/${code}/reverse`
    );
    return res;
  }

  async createNewDeposit(payload) {
    const res = await httpService.post("/api/accounting/deposits", payload);

    return res;
  }
  async getInstrumentTYpe() {
    const res = await httpService.get(
      "/api/administrator/maintain/gl-instrument-types"
    );
    return res;
  }
  async getAllBranch() {
    const res = await httpService.get("/api/administrator/maintain/branches");
    return res;
  }
  async getAllAccount(branchCode) {
    const res = await httpService.get(
      `/api/accounting/maintain/chart-of-accounts/children?branchCode=${branchCode}`
    );
    return res;
  }
  async getAllCustomer() {
    const res = await httpService.get("/api/customers");
    return res;
  }
  //getting all products
  async getAllProduct() {
    const res = await httpService.get("/api/accounting/products");
    return res;
  }
  //getting all Sub Account by Product
  async getAllSubAcctByProduct(customerName, productCode, branchCode) {
    console.log(productCode, branchCode);
    const res = await httpService.get(
      `/api/accounting/accounts/subsidiary?customerName=${customerName}&productCode=${productCode}&branchCode=${branchCode}`
    );
    return res;
  }
  //getAllCreditNote
  async getAllCreditNote(creditNoteStatus) {
    const res = await httpService.get(
      `api/accounting/credit-notes?status=${creditNoteStatus}`
    );
    return res;
  }

  //create new CreditNote
  async createNewCreditNote(payload) {
    const res = await httpService.post("/api/accounting/credit-notes", payload);

    return res;
  }

  // getAllDebit Note
  async getAllDebitNote(debitNoteStatus) {
    const res = await httpService.get(
      `/api/accounting/debit-notes?status=${debitNoteStatus}`
    );
    return res;
  }
  // create new debitNote
  async createNewDebitNote(payload) {
    const res = await httpService.post("/api/accounting/debit-notes", payload);

    return res;
  }

  // getAll customer transfer
  async getAllCustomerTransfer(customerTransferDataStatus) {
    const res = await httpService.get(
      `/api/accounting/transfers?filter.status=${customerTransferDataStatus}`
    );
    return res;
  }
  // create customer transfer
  async createCustomerTransfer(payload) {
    const res = await httpService.post("/api/accounting/transfers", payload);

    return res;
  }
  // getAll customer openingBal
  async getAllCustomerOpeningBal(openingBalStatus) {
    const res = await httpService.get(
      `/api/accounting/opening-balances?filter.status=${openingBalStatus}`
    );
    return res;
  }
  // create customer openingBal
  async createCustomerOpeningBal(payload) {
    const res = await httpService.post(
      "/api/accounting/opening-balances",
      payload
    );

    return res;
  }

  // get customer-balance
  async getCustomerBal(custAID, productCode) {
    const res = await httpService.get(
      `/api/accounting/customer/balance/${custAID}/${productCode}`
    );
    return res;
  }

  // get all self balancing
  async getAllSelfBalancing(selfBalancingStatus) {
    const res = await httpService.get(
      `api/accounting/journal/self?filter.status=${selfBalancingStatus}`
    );
    return res;
  }

  // create customer openingBal
  async createJournalSelfBalancing(payload) {
    const res = await httpService.post("/api/accounting/journal/self", payload);

    return res;
  }

  // get all batch posting list
  async getAllBatchPostingList(batchPostStatus) {
    const res = await httpService.get(
      `/api/accounting/journal/batch?filter.status=${batchPostStatus}`
    );
    return res;
  }
  //create batch post
  async createBatchPost(payload) {
    const res = await httpService.post(
      "/api/accounting/journal/batch",
      payload
    );

    return res;
  }
  //get single batch post
  async getSingleBatchPost(matserBatchId) {
    const res = await httpService.get(
      `/api/accounting/journal/batch/${matserBatchId}`
    );

    return res;
  }
  // all chart of accounts
  async getAllChartOfAccount() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts"
    );
    return res;
  }
  //get all account types
  async getAllAccountTypes() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts/types"
    );
    return res;
  }
  //get all account Levels
  async getAllAccountLevels() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts/levels"
    );
    return res;
  }
  // get All Parent with accountlevels and branchCode
  async getAllParentWithAccountLevelsAndBranchCode(branchCode, level) {
    const res = await httpService.get(
      `/api/accounting/maintain/chart-of-accounts/parent/${level}/${branchCode}`
    );
    return res;
  }

  //create maintain/chart of account

  async CreateMaintainChartOfAccount(payload) {
    const res = await httpService.post(
      "/api/accounting/maintain/chart-of-accounts",
      payload
    );

    return res;
  }

  //Statement Of Income
  async statementOfIncome() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts/ifrs/income-statement"
    );
    return res;
  }

  //Statement Of CashFlow
  async statementOfCashFlow() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts/ifrs/socf"
    );
    return res;
  }

  //Statement Of Financial Postion
  async statementOfFinancialPostion() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts/ifrs/sofp"
    );
    return res;
  }

  //Statement Of Changes In Equity
  async statementOfChangesInEquity() {
    const res = await httpService.get(
      "/api/accounting/maintain/chart-of-accounts/ifrs/socie"
    );
    return res;
  }
//get ALl GL params
async getAllGlParams() {
  const res = await httpService.get(
    "/api/accounting/maintain/params"
  );
  return res;
}

// update AllGlParams
async UpdateGlParams(payload) {
  const res = await httpService.post(
    "/api/accounting/maintain/params",
    payload
  );

  return res;
}

//get ALl Payment
async getAllPayment(status) {
  const res = await httpService.get(
    `/api/accounting/payments?filter.status=${status}`
  );
  return res;
}

async createNewPayment(payload) {
  const res = await httpService.post(
    "/api/accounting/payments",
    payload
  );

  return res;
}
  
}

export const DepositServices = new depositServices();
