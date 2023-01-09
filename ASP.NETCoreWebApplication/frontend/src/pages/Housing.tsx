import React, { useEffect, useState } from "react";
import axios from "axios";
import { getHousingObjects, getHousingObjectsMySQL, buildQuery, capitalize, navigateTo } from "../utils";
import { Button, Card, Checkbox, FormControlLabel, Grid, Switch, TextField } from "@mui/material";
import { SortComponent, ColoredLinearProgress, NextArrow, Layout, DropdownMenu } from "../components";
import { ALIO, ARUODAS, BUY_FLAT, BUY_HOUSE, DEFAULT_FILTER_VALUES, RENT_FLAT, RENT_HOUSE } from "../constants";

interface IHousingObject {
    title: string;
    url: string;
    price: number;
    location: string;
    currency: string;
    area: number;
    floorsMax: number;
    floorsThis: number;
    description: string;
    imgUrl: string;
}

export default function HousingPage(): JSX.Element {
    const [error, setError] = useState<Error | null>(null);
    const [housingObjects, setHousingObjects] = useState<IHousingObject[]>([]);
    const [searchKey, setSearchKey] = useState<string>(DEFAULT_FILTER_VALUES.SEARCH_STRING);
    const [priceMin, setPriceMin] = useState<number>(DEFAULT_FILTER_VALUES.PRICE_MIN);
    const [priceMax, setPriceMax] = useState<number>(DEFAULT_FILTER_VALUES.PRICE_MAX);
    const [roomsMin, setRoomsMin] = useState<number>(DEFAULT_FILTER_VALUES.ROOMS_MIN);
    const [roomsMax, setRoomsMax] = useState<number>(DEFAULT_FILTER_VALUES.ROOMS_MAX);
    const [floorsMin, setFloorsMin] = useState<number>(DEFAULT_FILTER_VALUES.FLOOR_MIN);
    const [floorsMax, setFloorsMax] = useState<number>(DEFAULT_FILTER_VALUES.FLOOR_MAX);
    const [areaMin, setAreaMin] = useState<number>(DEFAULT_FILTER_VALUES.AREA_MIN);
    const [areaMax, setAreaMax] = useState<number>(DEFAULT_FILTER_VALUES.AREA_MAX);
    const [fetching, setFetching] = useState<boolean>(DEFAULT_FILTER_VALUES.FETCHING_STATE);
    const [searchInDescription, setSeatchInDescription] = useState<boolean>(
        DEFAULT_FILTER_VALUES.SEARCH_IN_DESCRIPTION_STATE
    );
    const [dataSources, setDataSources] = useState<string[]>(DEFAULT_FILTER_VALUES.DATA_SOURCE);
    const [propertyType, setPropertyType] = useState<string>(DEFAULT_FILTER_VALUES.PROPERTY_TYPE);

    function removeDuplicatesOnURLKey(data1: IHousingObject[], data2: IHousingObject[]) {
        let primaryData: IHousingObject[] = [...data1];
        let urls: string[] = primaryData.map((obj: IHousingObject) => obj.url);
        data2.forEach((obj: IHousingObject) => {
            if (!urls.includes(obj.url)) {
                primaryData.push(obj);
            }
        });
        return primaryData;
    }

    function changeDataSources(event: React.ChangeEvent<HTMLInputElement>, checked: boolean) {
        if (checked) {
            setDataSources([...dataSources, event.target.value]);
        } else {
            setDataSources(dataSources.filter(source => source !== event.target.value));
        }
    }

    async function fetchLocation() {
        setFetching(true);
        return await new Promise((resolve, reject) => {
            navigator.geolocation.getCurrentPosition(
                position => {
                    setFetching(false);
                    resolve(position.coords);
                },
                (err: unknown) => {
                    setFetching(false);
                    setError(err as Error);
                }
            );
        });
    }

    async function getSavedRealEstate(query?: Record<string, any>) {
        setFetching(true);

        const response = await axios.get(getHousingObjectsMySQL + (query ? "?" + buildQuery(query) : ""));
        const data = await response.data;
        setHousingObjects(data);
        setFetching(false);
    }

    async function scrapeRealEstate(formBody: Record<string, any>) {
        setFetching(true);

        const response = await axios.post(getHousingObjects, formBody);
        const data = await response.data;
        let additionalData: IHousingObject[] = removeDuplicatesOnURLKey(housingObjects, data);

        setHousingObjects(additionalData);
        setFetching(false);
    }

    useEffect(() => {
        getSavedRealEstate({
            searchKey,
            priceMin,
            priceMax,
            roomsMin,
            roomsMax,
            floorsMin,
            floorsMax,
            searchInDescription,
            propertyType
        });
    }, [searchKey, priceMin, priceMax, roomsMin, roomsMax, floorsMin, floorsMax, searchInDescription, propertyType]);

    return (
        <Layout>
            <Card>
                <h4>Search & Filter:</h4>
                <div className={"form-group row pl-4 pr-4"}>
                    <div className={"col-lg-3 pw-search-form-group p-3"}>
                        <TextField
                            id="outlined-basic"
                            label="Search key"
                            variant="standard"
                            value={searchKey}
                            onChange={e => setSearchKey(capitalize(e.target.value))}
                        />
                        <FormControlLabel
                            control={
                                <Switch
                                    checked={searchInDescription}
                                    onChange={e => setSeatchInDescription(!searchInDescription)}
                                />
                            }
                            label="Search in description"
                            color={"secondary"}
                        />
                    </div>
                    <div className={"col-lg-3 pw-search-form-group p-3"}>
                        <TextField
                            id="outlined-basic"
                            label="Min price"
                            variant="standard"
                            value={priceMin}
                            onChange={e => setPriceMin(parseInt(e.target.value) || 0)}
                        />
                        <br />
                        <TextField
                            id="outlined-basic"
                            label="Max price"
                            variant="standard"
                            value={priceMax}
                            onChange={e => setPriceMax(parseInt(e.target.value) || 0)}
                        />
                    </div>
                    <div className={"col-lg-3 pw-search-form-group p-3"}>
                        <TextField id="outlined-basic" label="Floors min" variant="standard" />
                        <TextField id="outlined-basic" label="Floors max" variant="standard" />
                    </div>
                    <div className={"col-lg-3 pw-search-form-group p-3"}>
                        <TextField
                            id="outlined-basic"
                            label="Rooms min"
                            variant="standard"
                            value={roomsMin}
                            onChange={e => setRoomsMin(parseInt(e.target.value))}
                        />
                        <br />
                        <TextField
                            id="outlined-basic"
                            label="Rooms max"
                            variant="standard"
                            value={roomsMax}
                            onChange={e => setRoomsMax(parseInt(e.target.value))}
                        />
                    </div>
                    <Grid container>
                        <Grid item xs={4}>
                            <FormControlLabel
                                value={ARUODAS}
                                color={"secondary"}
                                control={<Checkbox defaultChecked color={"secondary"} onChange={changeDataSources} />}
                                label="Aruodas.lt"
                                labelPlacement="end"
                            />
                            <FormControlLabel
                                value={ALIO}
                                color={"secondary"}
                                control={<Checkbox defaultChecked color={"secondary"} onChange={changeDataSources} />}
                                label="Alio.lt"
                                labelPlacement="end"
                            />
                        </Grid>
                        <Grid item xs={4}>
                            <DropdownMenu
                                options={[BUY_FLAT, RENT_FLAT, BUY_HOUSE, RENT_HOUSE]}
                                displayName={["Buy flat", "Rent flat", "Buy house", "Rent house"]}
                                currentValue={propertyType}
                                outputCallback={setPropertyType}
                            />
                        </Grid>
                        <Grid item xs={4}>
                            <Button
                                onClick={() =>
                                    scrapeRealEstate({
                                        searchKey,
                                        priceMin,
                                        priceMax,
                                        roomsMin,
                                        roomsMax,
                                        areaMin,
                                        areaMax,
                                        floorsMin,
                                        floorsMax,
                                        dataSources: dataSources.join(","),
                                        propertyType
                                    })
                                }
                                variant={"outlined"}
                            >
                                Force scan
                            </Button>
                        </Grid>
                    </Grid>
                </div>
            </Card>
            <Card>
                <h4>Sort keys:</h4>
                <div className={"row pl-4 pr-4"}>
                    <SortComponent
                        array={housingObjects}
                        stateCallback={setHousingObjects}
                        predicate={() =>
                            (JSON.parse(JSON.stringify(housingObjects)) as IHousingObject[]).sort(
                                (h1: IHousingObject, h2: IHousingObject) => h1.price - h2.price
                            )
                        }
                        label={"Price"}
                    />
                </div>
            </Card>
            {fetching && <ColoredLinearProgress />}
            {error ? (
                <div className="alert alert-danger" role="alert">
                    <h4 className="alert-heading">Error!</h4>
                    {error.message}
                </div>
            ) : housingObjects.length ? (
                <div>
                    {housingObjects.map((house: IHousingObject) => (
                        <Card key={house.url}>
                            <Grid container>
                                <Grid xs={8} item>
                                    <Grid>
                                        <Grid xs={8} item>
                                            <h5>{house.title}</h5>
                                        </Grid>
                                        <Grid xs={3} item>
                                            <Button
                                                onClick={() => navigateTo(house.url)}
                                                endIcon={<NextArrow />}
                                                variant={"outlined"}
                                            >
                                                Explore
                                            </Button>
                                        </Grid>
                                    </Grid>
                                    <h5>
                                        <i className="fas fa-map-marked-alt"></i>
                                        {"  "}
                                        {house.location}
                                    </h5>
                                    <h5>
                                        <i className="fas fa-coins"></i>
                                        {"  "}
                                        {house.price} {house.currency}
                                    </h5>
                                    <h5>
                                        <i className="fas fa-building"></i>
                                        {"  "}
                                        {house.floorsThis}
                                        {"/"}
                                        {house.floorsMax}
                                    </h5>
                                </Grid>
                                <Grid xs={3} item>
                                    <img className="float-right" alt="" src={house.imgUrl} />
                                </Grid>
                            </Grid>
                        </Card>
                    ))}
                </div>
            ) : (
                "No results found"
            )}
        </Layout>
    );
}
