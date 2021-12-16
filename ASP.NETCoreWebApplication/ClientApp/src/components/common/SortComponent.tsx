import React, {Dispatch, useState} from "react";
import { Button } from "@material-ui/core";
import Purple from "./Purple";


enum SortState {
    ASCENDING=1,
    DESCENDING=-1,
    NONE=0
}

//we dont know what type of object we process there
type AsyncPredicate = (arg0: any, arg1: any) => Promise<any[]>;
type SyncPredicate = (arg0: any, arg1: any) => any[];

interface ISortComponentProps {
    predicate: SyncPredicate | AsyncPredicate;
    label: string;
    array: any[];
    stateCallback: Dispatch<any>;
}

function getIcon(sortState: SortState){
    switch (sortState) {
        case SortState.ASCENDING:
            return <i className="fas fa-arrow-up"></i>;
        case SortState.DESCENDING:
            return <i className="fas fa-arrow-down"></i>;
        case SortState.NONE:
            return <i className="fas fa-arrow-down d-none"></i>;
    }
}

export default function SortComponent(props: ISortComponentProps){
    const [sortState, setSortState] = useState<SortState>(SortState.NONE);
    function cycle(newState: SortState){
        switch(newState){
            case SortState.ASCENDING:
                setSortState(SortState.DESCENDING);
                return;
            case SortState.DESCENDING:
                setSortState(SortState.NONE);
                return;
            case SortState.NONE:
                setSortState(SortState.ASCENDING);
                return;
        }
    }
    return <Button variant={"outlined"} onClick={() => cycle(sortState)} endIcon={
        <Purple bold={true}>{getIcon(sortState)}</Purple>
    }>{props.label}</Button>
}