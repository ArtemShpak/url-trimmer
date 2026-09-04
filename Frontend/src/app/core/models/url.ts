export interface ShortUrl {
  id: number;
  originalUrl: string;
  shortCode: string;
  createdByUserId?: number;
  createdByUserName?: string;
  createdDate: string;
  clickCount?: number;
  canDelete?: boolean;
}
