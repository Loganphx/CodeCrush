import {User} from "./user";

export class UserParams {
  pageIndex: number = 1;
  pageSize: number = 5;
  gender: string;
  minAge: number = 18;
  maxAge: number = 99;
  orderBy: string = "lastActive";

  constructor(user: User) {
    this.gender = user.gender === "female" ? "male" : "female";
  }
}
