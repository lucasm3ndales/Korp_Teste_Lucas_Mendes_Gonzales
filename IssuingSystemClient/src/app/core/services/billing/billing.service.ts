import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IApiResult} from '../../../shared/models/default.model';
import {ICloseInvoiceNote, ICreateInvoiceNote, IInvoiceNote} from '../../../shared/models/invoice-note.model';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BillingService {
  private readonly baseUrl = 'http://localhost:6003/api/billing';
  private readonly invoiceNoteUrl = `${this.baseUrl}/v1/invoice-notes`;

  constructor(private http: HttpClient) {
  }

  getAllInvoiceNotes(): Observable<IApiResult<IInvoiceNote[]>> {
    return this.http.get<IApiResult<IInvoiceNote[]>>(this.invoiceNoteUrl);
  }

  getInvoiceNoteById(id: string): Observable<IApiResult<IInvoiceNote>> {
    return this.http.get<IApiResult<IInvoiceNote>>(`${this.invoiceNoteUrl}/${id}`);
  }

  createInvoiceNote(data: ICreateInvoiceNote): Observable<IApiResult<IInvoiceNote>> {
    return this.http.post<IApiResult<IInvoiceNote>>(this.invoiceNoteUrl, data);
  }

  closeInvoiceNote(data: ICloseInvoiceNote): Observable<IApiResult<boolean>> {
    return this.http.put<IApiResult<boolean>>(`${this.invoiceNoteUrl}/close`, data);
  }
}
