import React from "react";

declare interface IRowProps {
    children: React.ReactNode;
    fullWidth?: boolean;
    alignCenter?: boolean;
}

export default function (props: IRowProps): JSX.Element {
    return <div className={`row ${props.fullWidth ? "w-100" : ""} 
     ${props.alignCenter ? "align-content-center text-center" : ""}`}>{props.children}</div>
}