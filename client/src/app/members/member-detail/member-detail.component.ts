import {Component, OnInit, ViewChild} from '@angular/core';
import {MembersService} from "../../_services/members.service";
import {ActivatedRoute} from "@angular/router";
import {Member} from "../../_models/member";
import {CommonModule} from "@angular/common";
import {TabDirective, TabsetComponent, TabsModule} from "ngx-bootstrap/tabs";
import {GalleryItem, GalleryModule, ImageItem} from "ng-gallery";
import {TimeagoModule} from "ngx-timeago";
import {MemberMessagesComponent} from "../member-messages/member-messages.component";
import {MessageService} from "../../_services/message.service";
import {Message} from "../../_models/message";

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;
  member : Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab? :TabDirective;
  messages: Message[] = [];

  constructor(private membersService: MembersService,
              private route: ActivatedRoute,
              private messageService: MessageService) {
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages();
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages')
    {
      this.loadMessages();
    }
  }

  loadMessages() {
    this.messageService.getMessageThread(this.member!.username).subscribe({
      next: messages => this.messages = messages
    })
  }

  selectTab(heading: string) {
    if(this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading == heading)!.active = true;
    }
  }
  getImages(){
    if(!this.member) return;
    for (const photo of this.member.photos){
        this.images.push(new ImageItem({
          src: photo.url, thumb: photo.url
        }));
    }
  }
}
