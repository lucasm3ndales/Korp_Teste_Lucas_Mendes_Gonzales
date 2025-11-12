import {FormArray, FormControl, FormGroup} from '@angular/forms';
import {ICreateInvoiceNoteItem} from '../../../../../../shared/models/invoice-note-item.model';


export interface ICreateInvoiceNoteControls {
  items: FormArray<FormGroup<ICreateInvoiceNoteItemControls>>;
}


export interface ICreateInvoiceNoteItemControls {
  productId: FormControl<ICreateInvoiceNoteItem['productId']>;
  quantity: FormControl<ICreateInvoiceNoteItem['quantity']>;
  xmin: FormControl<ICreateInvoiceNoteItem['xmin']>;
}
