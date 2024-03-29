export const ARUODAS = "Aruodas.lt";
export const ALIO = "Alio.lt";

export const RENT_FLAT = "RENT_FLAT";
export const BUY_FLAT = "BUY_FLAT";
export const RENT_HOUSE = "RENT_HOUSE";
export const BUY_HOUSE = "BUY_HOUSE";

export const DEFAULT_FILTER_VALUES = {
    SEARCH_STRING: "",
    PRICE_MIN: 0,
    PRICE_MAX: 100000,
    ROOMS_MIN: 1,
    ROOMS_MAX: 5,
    FLOOR_MIN: 2,
    FLOOR_MAX: 10,
    AREA_MIN: 20,
    AREA_MAX: 200,
    FETCHING_STATE: false,
    SEARCH_IN_DESCRIPTION_STATE: false,
    DATA_SOURCE: [ARUODAS, ALIO],
    PROPERTY_TYPE: BUY_FLAT
};
