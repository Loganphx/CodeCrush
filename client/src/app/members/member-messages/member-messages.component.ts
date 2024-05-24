import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {Message} from "../../_models/message";
import {NgForOf, NgIf} from "@angular/common";
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
  ],
  styleUrls: ['./member-messages.component.scss']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm
  @Input() username?: string;
  @Input() messages: Message[] = [];
  messageContent = '';

  constructor(private messageService: MessageService) {
  }

  ngOnInit(): void {
  }

  sendMessage() {
    if (!this.username) return;

    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        console.log("Sending message " + this.messageContent + " to " + this.username);
        this.messages.push(message)
        this.messageForm?.reset()
      }
    })
  }
}
