import {Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {getPaginatedResult, getPaginationHeaders} from "./paginationHelper";
import {Message} from "../_models/message";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {User} from "../_models/user";
import {ToastrService} from "ngx-toastr";
import {BehaviorSubject, take} from "rxjs";
import {Group} from "../_models/group";
import {MembersService} from "./members.service";

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();
  messagesType?: string;
  count: number = 0;
  constructor(private http: HttpClient,
              private toastr: ToastrService,
              private memberService: MembersService) {
  }

  createHubConnection(user: User, otherUsername: string) {
    this.toastr.info("Creating Hub Connection")
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + "message?user=" + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => {
      console.log(error);
      this.toastr.error(error);
    });

      this.hubConnection!.on("ReceiveMessageThread", (messages: Message[]) => {
        this.messageThreadSource.next(messages);
      })

    this.hubConnection.on("UpdatedGroup", (group: Group) => {
      if(group.connections.some(x => x.username === otherUsername)) {
        this.memberService.getMember(otherUsername).pipe(take(1)).subscribe({
          next: member => {
            member.lastActive = new Date(Date.now())
          }
        })
        this.messageThreadSource.pipe(take(1)).subscribe({
          next: messages => {
            messages.forEach(message => {
              if(!message.dateRead) {
                message.dateRead = new Date(Date.now());
              }
            })
            this.messageThreadSource.next([...messages]);
          }
        })
      }
    })
      this.hubConnection!.on("NewMessage", message => {
        this.messageThread$.pipe(take(1)).subscribe({
          next: messages => {
            this.messageThreadSource.next([...messages, message]);
          }
        })
      })
  }

  stopHubConnection() {
    this.toastr.info("Stopping Hub Connection")
    if (this.hubConnection) {
      this.hubConnection?.stop().catch(error => {
        console.log(error);
        this.toastr.error(error);
      });
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container)

    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http)
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    // return this.http.post<Message>(this.baseUrl + 'messages', {recipientUsername: username, content})

    return this.hubConnection?.invoke("SendMessage", {
      recipientUsername: username, content
    }).catch(error => {
      console.log(error);
      this.toastr.error(error);
    });
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
