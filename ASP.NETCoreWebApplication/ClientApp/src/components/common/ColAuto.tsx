import React from "react";

declare interface IColAutoProps {
    pushOthersToRight?: boolean;
    pushToRight?: boolean;
    children: React.ReactNode;
}

export default function (props: IColAutoProps): JSX.Element {
    return <div className={`col-auto ${props.pushOthersToRight ? "mr-auto" : ""}
     ${props.pushToRight ? "mr-0" : ""}`}>{props.children}</div>
}