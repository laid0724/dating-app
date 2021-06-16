import { Component, Input, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/models/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.scss'],
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm: NgForm;

  @Input() username: string;
  // @Input() messages: Message[];
  messageContent: string;

  constructor(public messageService: MessageService) {}

  sendMessage(): void {
    // this.messageService
    //   .sendMessage(this.username, this.messageContent)
    //   .subscribe((message: Messages) => {
    //     this.messages.push(message);
    //     this.messageForm.reset();
    //   });

    this.messageService
      .sendMessageToHub(this.username, this.messageContent)
      .subscribe(() => {
        this.messageForm.reset();
      });
  }
}
