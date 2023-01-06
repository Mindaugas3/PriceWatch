import React from "react";

declare interface IRowProps {
    children: React.ReactNode;
    fullWidth?: boolean;
    alignCenter?: boolean;
}

export function Row(props: IRowProps): JSX.Element {
    return (
        <div
            className={`row ${props.fullWidth ? "w-100" : ""} 
     ${props.alignCenter ? "align-content-center text-center" : ""}`}
        >
            {props.children}
        </div>
    );
}
