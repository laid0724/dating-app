import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, from, Observable, of } from 'rxjs';
import { catchError, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from '../helpers';
import { Group } from '../models/group';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';
import { User } from '../models/users';

export type MessageContainer = 'Inbox' | 'Outbox' | 'Unread';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private endpoint = '/api/messages';
  private hubEndpoint = environment.hubUrl + '/message';
  private hubConnection: HubConnection;
  private messageThreadSource$ = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource$.asObservable();

  constructor(private http: HttpClient) {}

  createHubConnection(user: User, otherUserName: string): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubEndpoint}?user=${otherUserName}`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.error(error));

    this.hubConnection.on('ReceiveMessageThread', (messages: Message[]) => {
      this.messageThreadSource$.next(messages);
    });

    this.hubConnection.on('NewMessage', (message: Message) => {
      this.messageThreadSource$
        .pipe(take(1))
        .subscribe((messages: Message[]) => {
          this.messageThreadSource$.next([...messages, message]);
        });
    });

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some((c) => c.userName === otherUserName)) {
        this.messageThread$.pipe(take(1)).subscribe((messages) => {
          messages.forEach((message: Message) => {
            if (!message.dateRead) {
              message.dateRead = new Date(Date.now());
            }
            this.messageThreadSource$.next([...messages]);
          });
        });
      }
    });
  }

  stopHubConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((error) => console.error(error));
    }
  }

  getMessages(
    pageNumber: number,
    pageSize: number,
    container: MessageContainer
  ): Observable<PaginatedResult<Message[]>> {
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('container', container);

    return getPaginatedResult<Message[]>(this.http, this.endpoint, params);
  }

  getMessageThread(username: string): Observable<Message[]> {
    return this.http.get<Message[]>(this.endpoint + '/thread/' + username);
  }

  sendMessage(recipientUserName: string, content: string): Observable<Message> {
    return this.http.post<Message>(this.endpoint, {
      recipientUserName,
      content,
    });
  }

  sendMessageToHub(
    recipientUserName: string,
    content: string
  ): Observable<void> {
    return from(
      this.hubConnection.invoke('SendMessage', { recipientUserName, content })
    ).pipe(
      catchError((error) => {
        console.error(error);
        return of(error);
      })
    );
  }

  deleteMessage(id: number): Observable<null> {
    return this.http.delete<null>(this.endpoint + `/${id}`);
  }

  clearMessageThread(): void {
    this.messageThreadSource$.next([]);
  }
}
