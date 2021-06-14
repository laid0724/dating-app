import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { getPaginatedResult, getPaginationHeaders } from '../helpers';
import { Message } from '../models/message';
import { PaginatedResult } from '../models/pagination';

export type MessageContainer = 'Inbox' | 'Outbox' | 'Unread';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private endpoint = '/api/messages';

  constructor(private http: HttpClient) {}

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

  deleteMessage(id: number): Observable<null> {
    return this.http.delete<null>(this.endpoint + `/${id}`);
  }
}
