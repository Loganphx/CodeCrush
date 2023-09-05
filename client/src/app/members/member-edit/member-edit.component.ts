import {Component, HostListener, OnInit, ViewChild} from '@angular/core';
import {Member} from "../../_models/Member";
import {IUser} from "../../_models/IUser";
import {MembersService} from "../../_services/members.service";
import {AccountService} from "../../_services/account.service";
import {take} from "rxjs";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";

export interface MemberEditForm
{
  introduction: FormControl<string>;
  lookingFor: FormControl<string>;
  interests: FormControl<string>;
  city: FormControl<string>;
  country: FormControl<string>;
}
@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss']
})
export class MemberEditComponent implements OnInit {
  //@ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
    if(this.editMemberFormGroup.dirty) {
      $event.returnValue = true;
    }
  }

  member : Member | undefined;
  user : IUser | null = null;

  editMemberFormGroup = new FormGroup<MemberEditForm>({
    introduction: new FormControl('', {nonNullable:true,
      validators: [Validators.required]}),
    lookingFor: new FormControl('', {nonNullable:true,
      validators: [Validators.required]}),
    interests: new FormControl('', {nonNullable:true,
      validators: [Validators.required]}),
    city: new FormControl('', {nonNullable:true,
      validators: [Validators.required]}),
    country: new FormControl('', {nonNullable:true,
      validators: [Validators.required]}),
    });

    constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    });
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if(!this.user) return;
    this.memberService.getMember(this.user.username).subscribe({
      next: member => {
        this.member = member;
        this.editMemberFormGroup.controls.introduction.patchValue(member.introduction);
        this.editMemberFormGroup.controls.lookingFor.patchValue(member.lookingFor);
        this.editMemberFormGroup.controls.interests.patchValue(member.interests);
        this.editMemberFormGroup.controls.city.patchValue(member.city);
        this.editMemberFormGroup.controls.country.patchValue(member.country);

      }
    })
  }


  updateMember() {
      if(!this.member) return;
      this.member.introduction = this.editMemberFormGroup.controls.introduction.getRawValue();
      this.member.lookingFor = this.editMemberFormGroup.controls.lookingFor.getRawValue();
      this.member.interests = this.editMemberFormGroup.controls.interests.getRawValue();
      this.member.city = this.editMemberFormGroup.controls.city.getRawValue();
      this.member.country = this.editMemberFormGroup.controls.country.getRawValue();

      console.log(this.member);
      this.memberService.updateMember(this.member).subscribe({
        next: _ => {
          this.toastr.success('Profile updated successfully.')
          this.editMemberFormGroup.reset(this.member);
        }
      })
  }
}
