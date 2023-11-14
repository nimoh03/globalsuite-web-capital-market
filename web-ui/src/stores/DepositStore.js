import { makeAutoObservable } from "mobx";
import { DepositServices } from "../services/depositServices";
import NotificationService from "@/services/NotificationService";

class DepositStore {
  creditNoteStatus = "";
  debitNoteStatus = "";
  openingBalStatus = "";
  customerTransferDataStatus = ''

  constructor() {
    makeAutoObservable(this);
  }

  setCreditNoteStatus = (creditStatus) => {
    this.creditNoteStatus = creditStatus;
    localStorage.setItem("creditNoteStatus", creditStatus);
  };
  setDebitNoteStatus = (debitStatus) => {
    this.debitNoteStatus = debitStatus;
    localStorage.setItem("debitNoteStatus", debitStatus);
  };
  setOpeningBalStatus = (openingBalStatus) => {
    this.openingBalStatus = openingBalStatus;
    localStorage.setItem("openingBalStatus", openingBalStatus);
  };
  setCustomerTransfer = (customerTransferDataStatus) => {
    this.customerTransferDataStatus = customerTransferDataStatus;
    localStorage.setItem("customerTransferDataStatus", customerTransferDataStatus);
  };
  //getall deposit
  async getAlldeposit(status) {
    const response = await DepositServices.getAllDeposit(status);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occurred",
        "error"
      );
    }
    console.log(response.data.result);

    return response;
  }
  // get single deposit
  async getDepositByCode(payload) {
    const response = await DepositServices.getDepositByCode(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    return response;
  }
  async getPaymentByCode(payload) {
    const response = await DepositServices.getPaymentByCode(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    return response;
  }
  // get single CreditNote
  async getCreditNoteByCode(code, status) {
    const response = await DepositServices.getCreditNoteByCode(code, status);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    return response;
  }
  async getDebitNoteByCode(code, status) {
    console.log(status);
    const response = await DepositServices.getDebitNoteByCode(code, status);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    return response;
  }
  //get single opening bal
  async getOpeningBalanceByCode(code, status) {
   
    const response = await DepositServices.getOpeningBalanceByCode(
      code,
      status
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    return response;
  }
  async getCustomerTransferByCode(code, status) {
   
    const response = await DepositServices.getCustomerTransferByCode(
      code,
      status
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    return response;
  }
  //create new deposit
  async createNewDeposit(payload) {
    const response = await DepositServices.createNewDeposit(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occurred",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  // getAllInstrumentType
  async getInstrumentTYpe() {
    const response = await DepositServices.getInstrumentTYpe();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  // get branch
  async getAllBranch() {
    const response = await DepositServices.getAllBranch();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllAccount(branchCode) {
    console.log(branchCode);
    const response = await DepositServices.getAllAccount(branchCode);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllCustomer() {
    const response = await DepositServices.getAllCustomer();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  // approve transaction
  async approveTransaction(payload) {
    const response = await DepositServices.approveTransaction(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }
  async approvePaymentTransaction(payload) {
    const response = await DepositServices.approvePaymentTransaction(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }
  async approveCreditNoteTransaction(payload) {
    const response = await DepositServices.approveCreditNoteTransaction(
      payload
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }
  async approveCustomerOpeningBal(payload) {
    const response = await DepositServices.approveCustomerOpeningBal(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }
  async approveDebitNoteTransaction(payload) {
    const response = await DepositServices.approveDebitNoteTransaction(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }
  async approveCustomerTransfer(payload) {
    const response = await DepositServices.approveCustomerTransfer(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }

  //updating transaction
  async updateCustomerTransaction(payload) {
    const response = await DepositServices.updateCustomerTransaction(payload);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }

  async updateCreditNoteTransaction(payload) {
    const response = await DepositServices.updateCreditNoteTransaction(payload);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }
  async updatePaymentTransaction(payload) {
    const response = await DepositServices.updatePaymentTransaction(payload);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }

  async updateDebitNoteTransaction(payload) {
    const response = await DepositServices.updateDebitNoteTransaction(payload);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }
  //reverse transaction
  async reverseTransaction(payload) {
    const response = await DepositServices.reverseTransaction(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);

    return response;
  }
  async getAllProducts() {
    const response = await DepositServices.getAllProduct();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllSubAcctByProduct(customerName, productCode, branchCode) {
    const response = await DepositServices.getAllSubAcctByProduct(
      customerName,
      productCode,
      branchCode
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  // getAllCreditNote
  async getAllCreditNote(creditNoteStatus) {
    const response = await DepositServices.getAllCreditNote(creditNoteStatus);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  //create new creditNote
  async createNewCreditNote(payload) {
    const response = await DepositServices.createNewCreditNote(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }

  // get all debit Note
  async getAllDebitNote(debitNoteStatus) {
    const response = await DepositServices.getAllDebitNote(debitNoteStatus);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  // create new debit note
  async createNewDebitNote(payload) {
    const response = await DepositServices.createNewDebitNote(payload);

    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }

  // getAll customer transfer
  async getAllCustomerTransfer(customerTransferDataStatus) {
    const response = await DepositServices.getAllCustomerTransfer(
      customerTransferDataStatus
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  //CreateCustomer Transfer
  async createCustomerTransfer(payload) {
    const response = await DepositServices.createCustomerTransfer(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllCustomerOpeningBal(openingBalStatus) {
    const response = await DepositServices.getAllCustomerOpeningBal(
      openingBalStatus
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getCustomerBal(custAID, productCode) {
    const response = await DepositServices.getCustomerBal(custAID, productCode);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  //create new OpeningBAl
  async createNewOpeningBAl(payload) {
    const response = await DepositServices.createCustomerOpeningBal(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }

    console.log(response);
    return response;
  }

  async getAllSelfBalancing(selfBalancingStatus) {
    const response = await DepositServices.getAllSelfBalancing(
      selfBalancingStatus
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  //create Journal Self-Balancing
  async createJournalSelfBalancing(payload) {
    const response = await DepositServices.createJournalSelfBalancing(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  // get all batch post
  async getAllBatchPostingList(batchPostStatus) {
    const response = await DepositServices.getAllBatchPostingList(
      batchPostStatus
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  //Create Batch Post
  async createBatchPost(payload) {
    const response = await DepositServices.createBatchPost(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  // get single batch post
  async getSingleBatchPost(matserBatchId) {
    const response = await DepositServices.getSingleBatchPost(matserBatchId);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllChartOfAccount(batchPostStatus) {
    const response = await DepositServices.getAllChartOfAccount(
      batchPostStatus
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllAccountTypes() {
    const response = await DepositServices.getAllAccountTypes();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllAccountLevels() {
    const response = await DepositServices.getAllAccountLevels();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  async getAllParentWithAccountLevelsAndBranchCode(branchCode, level) {
    const response =
      await DepositServices.getAllParentWithAccountLevelsAndBranchCode(
        branchCode,
        level
      );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  //create maintain/chart of account
  async CreateMaintainChartOfAccount(payload) {
    const response = await DepositServices.CreateMaintainChartOfAccount(
      payload
    );
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async statementOfIncome() {
    const response = await DepositServices.statementOfIncome();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  async statementOfCashFlow() {
    const response = await DepositServices.statementOfCashFlow();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async statementOfFinancialPostion() {
    const response = await DepositServices.statementOfFinancialPostion();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  async statementOfChangesInEquity() {
    const response = await DepositServices.statementOfChangesInEquity();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async getAllGlParams() {
    const response = await DepositServices.getAllGlParams();
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async UpdateGlParams(payload) {
    const response = await DepositServices.UpdateGlParams(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
  async getAllPayment(status) {
    const response = await DepositServices.getAllPayment(status);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }

  async createNewPayment(payload) {
    const response = await DepositServices.createNewPayment(payload);
    if (response.kind !== "ok") {
      const { data } = response;
      NotificationService.show(
        data?.responseException?.exceptionMessage || "an error occured",
        "error"
      );
    }
    console.log(response);
    return response;
  }
}

export default DepositStore;
