import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../models/users';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  // this wont work because proxy conf wont capture web socket connections as it is not a http request!
  // private endpoint = '/hubs/presence';
  private endpoint = environment.hubUrl + '/presence';
  private hubConnection: HubConnection;
  private onlineUsersSource$ = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource$.asObservable();

  constructor(private toastr: ToastrService) {}

  createHubConnect(user: User): void {
    /*
      we need to manually send in our user token, as our jwt interceptor wont work
      because these are no longer http requests;
      also, websocket connection do not have support for http headers
    */

    // this configs and builds the hub connection
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.endpoint, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    // this actually starts the connection
    this.hubConnection.start().catch((error) => console.error(error));

    this.hubConnection.on('UserIsOnline', (username: string) => {
      this.toastr.info(`${username} has connected`);
    });

    this.hubConnection.on('UserIsOffline', (username: string) => {
      this.toastr.info(`${username} has disconnected`);
    });

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource$.next(usernames);
    });
  }

  stopHubConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((error) => console.error(error));
    }
  }
}
