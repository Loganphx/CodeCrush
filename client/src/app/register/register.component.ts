import {Component, EventEmitter, OnDestroy, OnInit, Output} from '@angular/core';
import {AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {AccountService} from "../_services/account.service";
import {ToastrService} from "ngx-toastr";
import {Subject, takeUntil} from "rxjs";
import {Router} from "@angular/router";

export interface RegisterForm
{
  username: FormControl<string>;
  email: FormControl<string>;
  knownAs: FormControl<string>;
  gender: FormControl<string>;
  dateOfBirth: FormControl<string>;
  city: FormControl<string>;
  country: FormControl<string>;
  password: FormControl<string>;
  confirmPassword: FormControl<string>;
}

export function matchValuesValidator(matchTo: string): ValidatorFn {
  return (control: AbstractControl) => {
    if(!control || !control.parent) return null;
    const matchControl = control.parent.get(matchTo)?.value;
    return control.value === matchControl ? null : {notMatching: true}
  }
}
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy{
  private _destroyed$: Subject<boolean> = new Subject<boolean>();

  public registerFormGroup: FormGroup<RegisterForm>;
  public maxDate = new Date();
  public validationErrors: string[] | undefined;

  //@Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter()


  constructor(private accountService: AccountService, private toastr: ToastrService,
              private formBuilder: FormBuilder, private router: Router) {
    this.registerFormGroup = this.formBuilder.group<RegisterForm>(
      {
        username: new FormControl('', {nonNullable:true,
          validators: [Validators.required]}),
        email: new FormControl('', {nonNullable:true,
          validators: [Validators.required,   Validators.email]}),
        knownAs: new FormControl('', {nonNullable:true,
          validators: [Validators.required, Validators.maxLength(64)]}),
        gender: new FormControl('', {nonNullable:true,
          validators: [Validators.required, Validators.maxLength(64)]}),
        dateOfBirth: new FormControl('', {nonNullable:true,
          validators: [Validators.required, Validators.maxLength(64)]}),
        city: new FormControl('', {nonNullable:true,
          validators: [Validators.required, Validators.maxLength(64)]}),
        country: new FormControl('', {nonNullable:true,
          validators: [Validators.required, Validators.maxLength(64)]}),
        password: new FormControl('', {nonNullable:true,
          validators: [Validators.required, Validators.minLength(8), Validators.maxLength(64)]}),
        confirmPassword: new FormControl('', {nonNullable:true, validators: [Validators.required, matchValuesValidator('password')]})
      });
  }

  ngOnInit(): void {
    this.registerFormGroup.controls.password.valueChanges
      .pipe(
        takeUntil(this._destroyed$),
        //tap(value => console.log("Tap", value))
      )
      .subscribe({
        next: () => this.registerFormGroup.controls.confirmPassword.updateValueAndValidity()
      })
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18)
  }

  ngOnDestroy(): void {
    this._destroyed$.next(true)
  }

  register()
  {
    const username: string = this.registerFormGroup.controls.username.getRawValue();
    const email: string = this.registerFormGroup.controls.email.getRawValue();
    const knownAs: string = this.registerFormGroup.controls.knownAs.getRawValue();
    const gender: string = this.registerFormGroup.controls.gender.getRawValue();
    const dateOfBirth = this.getDateOnly(this.registerFormGroup.controls.dateOfBirth.getRawValue());
    const city: string = this.registerFormGroup.controls.city.getRawValue();
    const country: string = this.registerFormGroup.controls.country.getRawValue();
    const password: string = this.registerFormGroup.controls.password.getRawValue();

    const values = {...this.registerFormGroup.value, dateOfBirth:dateOfBirth};
    const model = {
      "username":username,
      "email":email,
      "knownAs":knownAs,
      "gender":gender,
      "dateOfBirth":dateOfBirth,
      "city":city,
      "country":country,
      "password":password
    };

    this.accountService.register(values).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
      },
      error: error => {
        console.log(error)
        this.toastr.error(error.error.errors)
        this.validationErrors = error;
      }
    })
  }

  cancel()
  {
    this.cancelRegister.emit(false)
  }

  private getDateOnly(dob: string | undefined) {
    if(!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset()))
      .toISOString().slice(0,10)
  }
}
