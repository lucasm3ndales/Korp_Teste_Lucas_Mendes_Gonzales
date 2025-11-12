export interface IProduct {
  id: string;
  code: string;
  description: string;
  stockBalance: number;
  xmin: string;
  createdAt: string;
}

export interface ICreateProduct {
  code: string;
  description: string;
  initialStockBalance: number;
}
