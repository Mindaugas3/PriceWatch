import React, { useEffect, useState } from "react";
import { getHousingObjects, getHousingObjectsMySQL } from "../utils/route-paths";
import { Button, Card, Checkbox, FormControlLabel } from "@mui/material";
import { Row, NextArrow, ColoredLinearProgress, Layout } from "../components";
import { DEFAULT_FILTER_VALUES } from "../constants";

interface IItemObject {
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

function capitalize(input: string): string {
    const [first, ...rest] = input.split("");
    if (input === "") return "";
    if (input.length === 1) return first.toUpperCase();
    return first.toUpperCase() + rest.map((r: string) => r.toLowerCase()).join("");
}

export default function ItemsPage(): JSX.Element {
    const [error, setError] = useState<Error | null>(null);
    const [housingObjects, setHousingObjects] = useState<IItemObject[]>([]);
    const [searchKey, setSearchKey] = useState<string>(DEFAULT_FILTER_VALUES.SEARCH_STRING);
    const [priceMin, setPriceMin] = useState<number>(DEFAULT_FILTER_VALUES.PRICE_MIN);
    const [priceMax, setPriceMax] = useState<number>(DEFAULT_FILTER_VALUES.PRICE_MAX);
    const [fetching, setFetching] = useState<boolean>(DEFAULT_FILTER_VALUES.FETCHING_STATE);
    const params: RequestInit = { headers: { "Content-Type": "application/json" } };

    function removeDuplicatesOnURLKey(data1: IItemObject[], data2: IItemObject[]) {
        let primaryData: IItemObject[] = [...data1];
        let urls: string[] = primaryData.map((obj: IItemObject) => obj.url);
        data2.forEach((obj: IItemObject) => {
            if (!urls.includes(obj.url)) {
                primaryData.push(obj);
            }
        });
        return primaryData;
    }

    async function fetchLocation() {
        setFetching(true);
        return await new Promise(resolve => {
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
        //
    }

    async function navigateTo(src: string) {
        window.open(src, "_blank");
    }

    async function fetchHousingObjectList(t?: any) {
        setFetching(true);

        const response = await fetch(
            getHousingObjectsMySQL +
                (params
                    ? "?" +
                      Object.keys(t)
                          .reduce((prevStr: string, param: string) => {
                              return prevStr + param + "=" + t[param] + "&";
                          }, "")
                          .slice(0, -1)
                    : "")
        );
        const data = await response.json();
        setHousingObjects(data);
        setFetching(false);
    }

    async function fetchHousingObjectListScrapper(t?: any) {
        setFetching(true);

        const response = await fetch(
            getHousingObjects +
                (params
                    ? "?" +
                      Object.keys(t)
                          .reduce((prevStr: string, param: string) => {
                              return prevStr + param + "=" + t[param] + "&";
                          }, "")
                          .slice(0, -1)
                    : "")
        );
        const data = await response.json();
        let additionalData: IItemObject[] = removeDuplicatesOnURLKey(housingObjects, data);

        setHousingObjects(additionalData);
        setFetching(false);
    }

    useEffect(() => {
        fetchHousingObjectList({
            searchKey,
            priceMin,
            priceMax
        });
    }, [searchKey, priceMin, priceMax]);

    return (
        <Layout>
            <Card>
                <h4>Search & Filter:</h4>
                <div className={"form-group row pl-4 pr-4"}>
                    <div className={"col-lg-4"}>
                        Search key{" "}
                        <input
                            type={"text"}
                            name={"searchKey"}
                            value={searchKey}
                            onChange={e => setSearchKey(capitalize(e.target.value))}
                        />
                    </div>
                    <div className={"col-lg-4"}>
                        Min price{" "}
                        <input
                            type={"text"}
                            name={"price"}
                            value={priceMin}
                            onChange={e => setPriceMin(parseInt(e.target.value) || 0)}
                        />
                    </div>
                    <div className={"col-lg-4"}>
                        Max price{" "}
                        <input
                            type={"text"}
                            name={"price"}
                            value={priceMax}
                            onChange={e => setPriceMax(parseInt(e.target.value) || 0)}
                        />
                    </div>
                    <div className={"w-100"}></div>
                    <div className={"w-100"}></div>
                    <div className={"row pl-4 pr-4 w-100"}>
                        <div className={"col-auto mr-auto"}>
                            <FormControlLabel
                                value="aruodas"
                                color={"secondary"}
                                control={<Checkbox />}
                                label="Aruodas.lt"
                                labelPlacement="end"
                            />
                            <FormControlLabel
                                value="alio"
                                color={"secondary"}
                                control={<Checkbox />}
                                label="Alio.lt"
                                labelPlacement="end"
                            />
                        </div>

                        <div className={"col-auto"}>
                            <Button
                                onClick={() =>
                                    fetchHousingObjectListScrapper({
                                        searchKey,
                                        priceMin,
                                        priceMax
                                    })
                                }
                                variant={"outlined"}
                            >
                                Force scan
                            </Button>
                        </div>
                    </div>
                </div>
            </Card>
            <Card>
                <h4>Sort keys:</h4>
                <div className={"row pl-4 pr-4"}>
                    <button>Price</button>
                    <button>Newest fetched</button>
                    <button onClick={fetchLocation}>Location (km, relative)</button>
                </div>
            </Card>
            {fetching && <ColoredLinearProgress />}
            {error ? (
                <div className="alert alert-danger" role="alert">
                    <h4 className="alert-heading">Error!</h4>
                    {error.message}
                </div>
            ) : (
                housingObjects.length && (
                    <div>
                        {housingObjects.map((house: IItemObject) => (
                            <Card>
                                <Row>
                                    {/* <ColAuto pushOthersToRight>
                            <Row fullWidth>
                                <ColAuto pushOthersToRight>
                                    <h5>{house.title}</h5>
                                </ColAuto>
                                <ColAuto pushToRight={true}>
                                    <Button onClick={() => navigateTo(house.url)} endIcon={<Next />} variant={"outlined"}>
                                        Explore
                                    </Button>
                                </ColAuto>
                            </Row>
                            <h5><i className="fas fa-map-marked-alt"></i>{'  '}{house.location}</h5>
                            <h5><i className="fas fa-coins"></i>{'  '}{house.price}{' '}{house.currency}</h5>
                            <h5><i className="fas fa-building"></i>{'  '}{house.floorsThis}{'/'}{house.floorsMax}</h5>
                        </ColAuto>
                        <ColAuto >
                            <img className="float-right" src={house.imgUrl}/>
                        </ColAuto> */}
                                </Row>
                            </Card>
                        ))}
                    </div>
                )
            )}
        </Layout>
    );
}
