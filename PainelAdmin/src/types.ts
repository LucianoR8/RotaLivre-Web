export interface AdminUser {
  id: number;
  name: string;
  email: string;
  role: string;
  avatarUrl?: string;
}

export interface Category {
  id: number;
  name: string;
  imageUrl: string;
  description?: string;
  tourCount?: number;
  isActive: boolean;

  audit?: {
    lastEditedBy: string;
    lastEditedAt: string;
  };
}

export interface AvailabilityBlock {
  id: string | number;
  date: string;
  timeSlots: string[];
}

export interface TourLocation {
  latitude: number;
  longitude: number;
}

export interface TourAddress {
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  zipCode: string;
}

export interface Tour {
  id: number;
  name: string;
  categoryId: number;
  categoryName?: string;
  photoUrl: string;
  description: string;
  operatingDescription: string;

  location?: TourLocation;
  address?: TourAddress;

  availabilities?: AvailabilityBlock[];

  rating?: number;
  reviewsCount?: number;

  status: 'ativo' | 'inativo' | 'rascunho';

  audit?: {
    lastEditedBy: string;
    lastEditedAt: string;
  };
}

export interface Review {
  id: number;
  tourId: number;
  tourName: string;
  userName: string;
  userAvatar: string;
  rating: number;
  comment: string;
  date: string;
}

export interface DashboardStats {
  totalTours: number;
  totalCategories: number;
  newReviews: number;
  activeBookings: number;
  monthlyRevenue: number;
}

export type ActiveScreen =
  | 'dashboard'
  | 'categories'
  | 'category-new'
  | 'category-edit'
  | 'tours'
  | 'tour-new'
  | 'tour-edit';