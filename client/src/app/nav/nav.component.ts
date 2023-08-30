import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validator, Validators} from "@angular/forms";
import {AccountService} from "../_services/account.service";
import {Observable} from "rxjs";
import {IUser} from "../_models/IUser";
import {Router} from "@angular/router";
import {ToastrService} from "ngx-toastr";

export interface LoginForm
{
  username: FormControl<string>
  password: FormControl<string>
}

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit
{
  formGroup = new FormGroup<LoginForm>(
    {
      username: new FormControl('', {nonNullable:true,
        validators: [Validators.required, Validators.minLength(6)]}),
      password: new FormControl('', {nonNullable:true,
        validators: [Validators.required, Validators.minLength(8)]}),
    });


  constructor(public accountService: AccountService, private router: Router) {
  }

  ngOnInit(): void {
  }

  login() {
    const username = this.formGroup.controls.username.getRawValue();
    const password = this.formGroup.controls.password.getRawValue();
    this.accountService.login({"Username": username, "Password": password})
      .subscribe({
        next: _ => this.router.navigateByUrl("/members"),
        //error: error => this.toastr.error(error.error)
    });
  }

  logout()
  {
    this.accountService.logout();
    this.router.navigateByUrl("/");
  }

}
