import React, {useEffect, useState} from "react";
import {Layout} from "../components/Layout";
import {getHousingObjectsMySQL} from "../utils/RoutePaths";
import {Button, Card, Checkbox, FormControlLabel} from "@material-ui/core";
import LinearProgress from "../components/common/LinearProgress";

//import '../../../node_modules/bootstrap/dist/css/bootstrap.min.css';

interface IHousingObject {
    title: string,
    url: string,
    price: number,
    location: string,
    currency: string,
    area: number,
    floorsMax: number,
    floorsThis: number,
    description: string,
    imgUrl: string
}



export default function (props: any): JSX.Element {
    const [error, setError] = useState<Error | PositionError | null>(null);
    const [housingObjects, setHousingObjects] = useState<IHousingObject[]>([]);
    //search keys
    const [searchKey, setSearchKey] = useState<string>('');
    const [priceMin, setPriceMin] = useState<number>(0);
    const [priceMax, setPriceMax] = useState<number>(100000);
    const [roomsMin, setRoomsMin] = useState<number>(0);
    const [roomsMax, setRoomsMax] = useState<number>(5);
    const [floors, setFloors] = useState<number>(2);
    const params: RequestInit = { headers: {'Content-Type': 'application/json'} };
    
    async function fetchLocation() {
        return await new Promise((resolve, reject) => {
            navigator.geolocation.getCurrentPosition((position) => {
                resolve(position.coords);
            }, (err) => {
                setError(err);
            });
        });
        //
    }
    
    async function navigateTo(src: string){
        window.location.href = src;
    }

    async function fetchHousingObjectList(t?: any) {
        // const token = await authService.getAccessToken();
        // {
        //     headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        // }
        
        const response = await fetch(getHousingObjectsMySQL + (
            params ? '?' +Object.keys(t).reduce((prevStr: string, param: string) => {
                return prevStr + param + '=' + t[param] + '&';
            }, '').slice(0, -1) : ''
        ));
        const data = await response.json();
        setHousingObjects(data);
    }
    
    useEffect(() => {
        fetchHousingObjectList({
            searchKey,
            priceMin,
            priceMax,
            roomsMin,
            roomsMax,
            floors
        });
    }, []);

    // return <div>{JSON.stringify(props)}</div>;
    return <Layout>
        <Card>
            <h4>Search & Filter:</h4>
        <div className={'form-group row pl-4 pr-4'}>
            <div className={"col-lg-4"}>
                Search key {' '} <input type={"text"} name={"searchKey"} value={searchKey} onChange={(e) => setSearchKey(e.target.value)}/>
            </div>
            <div className={"col-lg-4"}>
                Min price {' '} <input type={"text"} name={"price"} value={priceMin} onChange={(e) => setPriceMin(parseInt(e.target.value))}/>
            </div>
            <div className={"col-lg-4"}>
                Max price {' '} <input type={"text"} name={"price"} value={priceMax} onChange={(e) => setPriceMax(parseInt(e.target.value))}/>
            </div>
            <div className={"w-100"}>
                
            </div>
            <div className={"col-lg-4"}>
                Rooms min {' '} <input type={"number"} value={roomsMin} onChange={(e) => setRoomsMin(parseInt(e.target.value))}/>
            </div>
            <div className={"col-lg-4"}>
                Rooms max {' '} <input type={"number"} value={roomsMax} onChange={(e) => setRoomsMax(parseInt(e.target.value))}/>
            </div>
            <div className={"w-100"}>

            </div>
            <div className={"row pl-4 pr-4 w-100"}>
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
                <Button className={"float-right"}>Force scan</Button>
            </div>
        </div>
    </Card>
        <Card>
            <h4>Sort keys:</h4>
        <div className={'row pl-4 pr-4'}>
            <button>Price</button>
            <button>Newest fetched</button>
            <button onClick={fetchLocation}>Location (km, relative)</button>
        </div>
        </Card>
        {error ? <div className="alert alert-danger" role="alert">
            <h4 className="alert-heading">Error!</h4>
            {error.message}
        </div> : (housingObjects.length) ? (<div>
            {housingObjects.map((house: IHousingObject) => (
                <Card>
                    <h4>{house.title}</h4>
                    <i className="fas fa-map-marked-alt"></i><h5>{house.location}</h5>
                    <i className="fas fa-coins"></i> <h5>{house.price}{' '}{house.currency}</h5>
                    <i className="fas fa-building"></i><h5>{house.floorsThis}{'/'}{house.floorsMax}</h5>
                    <img className="float-right" src={house.imgUrl}/>
                    <Button onClick={() => navigateTo(house.url)}>Explore</Button>
                </Card>
            ))}
        </div>) : (<div className="w-100 h-100">
            <LinearProgress/>
        </div>)}
    </Layout>
}