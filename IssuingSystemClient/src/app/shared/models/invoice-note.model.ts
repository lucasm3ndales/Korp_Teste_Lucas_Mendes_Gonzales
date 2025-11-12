import {ICreateInvoiceNoteItem, IInvoiceNoteItem} from './invoice-note-item.model';


export interface IInvoiceNote {
  id: string;
  noteNumber: number;
  status: InvoiceNoteStatus;
  createdAt: string;
  updatedAt: string;
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
}

export const translateInvoiceNoteStatus = (value: InvoiceNoteStatus): string => {
  const statusMap: Record<InvoiceNoteStatus, string> = {
    [InvoiceNoteStatus.OPEN]: 'Aberta',
    [InvoiceNoteStatus.CLOSED]: 'Fechada',
    [InvoiceNoteStatus.PROCESSING]: 'Em Processamento',
  };

  return statusMap[value] || 'Desconhecido';
};
