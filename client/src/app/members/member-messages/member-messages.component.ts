import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { NgForm } from '@angular/forms';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.scss'],
})
export class MemberMessagesComponent implements OnDestroy {
  @ViewChild('messageForm') messageForm: NgForm;

  // see: https://stackoverflow.com/questions/39366981/viewchild-in-ngif
  private _chatMessagesContainer: ElementRef<HTMLDivElement>;
  @ViewChild('chatMessagesContainer', { static: false })
  set chatMessagesContainer(chatMessagesContainer: ElementRef<HTMLDivElement>) {
    // initially setter gets called with undefined
    if (chatMessagesContainer) {
      this._chatMessagesContainer = chatMessagesContainer;

      const chatMessagesContainerEl: HTMLDivElement =
        this._chatMessagesContainer.nativeElement;

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

  ngOnDestroy(): void {
    // this is so that remnants of the last chat with another user
    // wont show up when you swap to chat with another user
    this.messageService.clearMessageThread();
  }
}
