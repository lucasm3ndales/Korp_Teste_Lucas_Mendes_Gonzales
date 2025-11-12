import {FormControl} from '@angular/forms';
import {ICloseInvoiceNote} from '../../../../../../shared/models/invoice-note.model';


export interface ICloseInvoiceNoteControls {
  id: FormControl<ICloseInvoiceNote['id'] | null>;
  xmin: FormControl<ICloseInvoiceNote['xmin'] | null>;
}
