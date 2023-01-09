import React, { Dispatch, useState } from "react";
import { Button } from "@mui/material";

enum SortState {
    ASCENDING = 1,
    DESCENDING = -1,
    NONE = 0
}

//we dont know what type of object we process there
type SyncPredicate = () => any[];

interface ISortComponentProps {
    predicate: SyncPredicate;
    label: string;
    array: any[];
    stateCallback: Dispatch<any>;
}

function getIcon(sortState: SortState) {
    switch (sortState) {
        case SortState.ASCENDING:
            return <i className="fas fa-arrow-up"></i>;
        case SortState.DESCENDING:
            return <i className="fas fa-arrow-down"></i>;
        case SortState.NONE:
            return <i className="fas fa-arrow-down d-none"></i>;
    }
}

async function execPredicate(predicate: SyncPredicate, array: any[], stateCallback: Dispatch<any>, state: SortState) {
    if (state === SortState.NONE) {
        return array;
    } else if (state === SortState.ASCENDING) {
        return stateCallback(predicate());
    } else {
        return stateCallback(predicate().reverse());
    }
}

export function SortComponent(props: ISortComponentProps) {
    const [sortState, setSortState] = useState<SortState>(SortState.NONE);
    function cycle(newState: SortState) {
        switch (newState) {
            case SortState.ASCENDING:
                setSortState(SortState.DESCENDING);
                execPredicate(props.predicate, props.array, props.stateCallback, newState);
                return;
            case SortState.DESCENDING:
                setSortState(SortState.NONE);
                execPredicate(props.predicate, props.array, props.stateCallback, newState);
                return;
            case SortState.NONE:
                setSortState(SortState.ASCENDING);
                execPredicate(props.predicate, props.array, props.stateCallback, newState);
                return;
        }
    }
    return (
        <Button
            variant={"outlined"}
            onClick={() => cycle(sortState)}
            endIcon={
                <b>
                    <span className="purple">{getIcon(sortState)}</span>
                </b>
            }
        >
            {props.label}
        </Button>
    );
}
