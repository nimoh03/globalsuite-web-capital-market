import { makeAutoObservable } from "mobx";

class UserStore {
  firstName = "John";

  constructor() {
    makeAutoObservable(this);
  }

  setFirstName(firstName: string) {
    this.firstName = firstName;
  }
}

export default UserStore;
