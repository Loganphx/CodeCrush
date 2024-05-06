import {User} from "./user";

export class PaginationParams {
  pageIndex: number = 1;
  pageSize: number = 5;
}

export class UserParams extends PaginationParams {
  gender: string;
  minAge: number = 18;
  maxAge: number = 99;
  orderBy: string = "lastActive";

  constructor(user: User) {
    super()
    this.gender = user.gender === "female" ? "male" : "female";
  }
}

export class LikesParams extends PaginationParams {
  predicate: string = "liked";
}
