import React from "react";
import UserStore from "./userStore";
import DepositStore from './DepositStore'
import AuthStore from "./authStore";
import AccountingPaymentsStore from "./accountingPaymentsStore";

export const stores = Object.freeze({
  userStore: new UserStore(),
  depositStore : new DepositStore(),
  authStore: new AuthStore(),
  accountingPaymentsStore: new AccountingPaymentsStore(),
});

export const rootContext = React.createContext(stores);

export const RootProvider = rootContext.Provider;
