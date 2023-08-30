import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {AccountService} from "../_services/account.service";
import {ToastrService} from "ngx-toastr";

export interface RegisterForm
{
  username: FormControl<string>;
  email: FormControl<string>
  password: FormControl<string>;
}
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit{
  //@Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter()
  model: any = {};

  registerFormGroup = new FormGroup<RegisterForm>(
    {
    username: new FormControl('', {nonNullable:true,
      validators: [Validators.required, Validators.minLength(6)]}),
    email: new FormControl('', {nonNullable:true,
      validators: [Validators.required,   Validators.email]}),
    password: new FormControl('', {nonNullable:true,
      validators: [Validators.required, Validators.minLength(8)]}),
  });

  constructor(private accountService: AccountService, private toastr: ToastrService) {
  }

  ngOnInit(): void {
  }

  register()
  {
    const username = this.registerFormGroup.controls.username.getRawValue();
    const email = this.registerFormGroup.controls.email.getRawValue();
    const password = this.registerFormGroup.controls.password.getRawValue();

    this.accountService.register({"username":username, "email":email, "password":password}).subscribe({
      next: () => {
        this.cancel();
      },
      error: error => {
        console.log(error)
        this.toastr.error(error.error.errors)
      }
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false)
  }

}
