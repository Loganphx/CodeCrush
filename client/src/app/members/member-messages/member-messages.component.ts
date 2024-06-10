import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {AsyncPipe, NgForOf, NgIf} from "@angular/common";
import {SharedModule} from "../../_modules/shared.module";
import {MessageService} from "../../_services/message.service";
import {FormsModule, NgForm} from "@angular/forms";
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  standalone: true,
  imports: [
    NgForOf,
    SharedModule,
    NgIf,
    FormsModule,
    TimeagoModule,
    AsyncPipe,
  ],
  styleUrls: ['./member-messages.component.scss']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;
  messageContent = '';

  constructor(public messageService: MessageService) {
  }

  ngOnInit(): void {
  }

  sendMessage() {
    if (!this.username) return;

    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    });
  }
}
