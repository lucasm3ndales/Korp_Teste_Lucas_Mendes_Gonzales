export interface IInvoiceNoteItem {
  id: number;
  productId: string;
  productCode: string;
  productDescription: string;
  quantity: number;
}

export interface ICreateInvoiceNoteItem {
  productId: string;
  quantity: number;
  xmin: number;
}
