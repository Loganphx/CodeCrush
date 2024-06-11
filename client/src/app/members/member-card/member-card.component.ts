import {Component, Input, OnInit} from '@angular/core';
import {Member} from "../../_models/member";
import {MembersService} from "../../_services/members.service";
import {ToastrService} from "ngx-toastr";
import {PresenceService} from "../../_services/presence.service";
import {environment} from "../../../environments/environment";

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.scss'],
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member | undefined;
  defaultProfilePictureUrl: string = environment.defaultProfilePictureUrl;

  constructor(private memberService: MembersService,
              private toastr: ToastrService,
              public presenceService: PresenceService) {
  }

  ngOnInit(): void {
  }

  addLike(member: Member) {
    this.memberService.addLike(member.username).subscribe({
      next: () => this.toastr.success('You have liked ' + member.knownAs)
    })
  }
}
