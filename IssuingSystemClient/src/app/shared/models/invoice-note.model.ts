import {ICreateInvoiceNoteItem, IInvoiceNoteItem} from './invoice-note-item.model';


export interface IInvoiceNote {
  id: string;
  noteNumber: number;
  status: InvoiceNoteStatus;
  createdAt: string;
  updatedAt: string;
  rowVersion: number;
  items: IInvoiceNoteItem[];
}

export enum InvoiceNoteStatus {
  OPEN = 'OPEN',
  CLOSED = 'CLOSED',
  PROCESSING = 'PROCESSING',
}

export interface ICreateInvoiceNote {
  items: ICreateInvoiceNoteItem[]
}

export interface ICloseInvoiceNote {
  id: string;
  rowVersion: number;
}
