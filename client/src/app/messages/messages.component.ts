import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';
import { Message } from '../models/message';
import { PaginatedResult, Pagination } from '../models/pagination';
import { MessageContainer, MessageService } from '../services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss'],
})
export class MessagesComponent implements OnInit, OnDestroy {
  messages: Message[] = [];
  pagination: Pagination;
  container: MessageContainer = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  messagesRefresher$ = new Subject<MessageContainer>();
  destroyer$ = new Subject<boolean>();

  constructor(private messageService: MessageService) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  resetPage(): void {
    this.pageNumber = 1;
    this.pagination.currentPage = 1;
  }

  refreshMessages(container: MessageContainer): void {
    this.messagesRefresher$.next(container);
  }

  loadMessages(config?: { resetPage: boolean }): void {
    if (config && config.resetPage) {
      this.resetPage();
    }
    this.loading = true;
    this.messagesRefresher$
      .pipe(
        startWith(this.container),
        switchMap((container: MessageContainer) =>
          this.messageService.getMessages(
            this.pageNumber,
            this.pageSize,
            container
          )
        ),
        takeUntil(this.destroyer$)
      )
      .subscribe((response: PaginatedResult<Message[]>) => {
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading = false;
      });
  }

  deleteMessage(id: number): void {
    this.messageService.deleteMessage(id).subscribe(() => {
      this.messages.splice(
        this.messages.findIndex((m) => m.id === id),
        1
      );
    });
  }

  onPageChanged(pageChangeEvent: { page: number; itemsPerPage: number }): void {
    this.pageNumber = pageChangeEvent.page;
    this.refreshMessages(this.container);
  }

  ngOnDestroy(): void {
    this.destroyer$.next(true);
    this.destroyer$.unsubscribe();
  }
}
