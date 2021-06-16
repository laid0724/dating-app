import {
  Component,
  ElementRef,
  Input,
  Renderer2,
  ViewChild,
} from '@angular/core';
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

  // see: https://stackoverflow.com/questions/39366981/viewchild-in-ngif
  private chatMessagesContainer: ElementRef<HTMLDivElement>;
  @ViewChild('chatMessagesContainer', { static: false }) set content(
    content: ElementRef<HTMLDivElement>
  ) {
    if (content) {
      // initially setter gets called with undefined
      this.chatMessagesContainer = content;

      const chatMessagesContainerEl: HTMLDivElement =
        this.chatMessagesContainer.nativeElement;

      // scroll to bottom of the chat window
      this.renderer.setProperty(
        chatMessagesContainerEl,
        'scrollTop',
        chatMessagesContainerEl.scrollHeight
      );
    }
  }

  @Input() username: string;
  // @Input() messages: Message[];
  messageContent: string;

  constructor(
    public messageService: MessageService,
    private renderer: Renderer2
  ) {}

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
