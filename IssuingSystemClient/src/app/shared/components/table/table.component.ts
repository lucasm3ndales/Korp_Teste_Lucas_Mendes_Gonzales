import {Component, ContentChildren, Input, QueryList, TemplateRef,} from '@angular/core';
import {IColumn} from './models/table.model';
import {TableModule} from 'primeng/table';
import {NgStyle, NgTemplateOutlet} from '@angular/common';


@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    TableModule,
    NgStyle,
    NgTemplateOutlet,
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.css',
})
export class TableComponent<T> {
  @Input() columns: IColumn[] = [];
  @Input() rows: T[] = [];
  @ContentChildren('detailsTemplate', { read: TemplateRef })
  public detailsTemplate!: QueryList<TemplateRef<any>>;
  @ContentChildren('statusTemplate', { read: TemplateRef })
  public statusTemplate!: QueryList<TemplateRef<any>>;
}
