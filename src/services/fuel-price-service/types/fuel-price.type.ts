export interface IFuelPrice {
    identifier: string;
    name: string;
    address: string;
    postcode: string;
    provider: string;
    brand: string;
    latitude: number | null;
    longitude: number | null;
    petrol_e5_price: number | null;
    petrol_e10_price: number | null;
    diesel_b7_price: number | null;
    updatedAt: string;
    createdAt: string;
    distanceInMeters: number;

    colour: string;
    logo: string;
}
