import React from "react";
import "./PwCss.css";
import { ColoredElementProps } from "./CommonTypes";


const Purple: (props: ColoredElementProps) => JSX.Element = ({children, bold = false}) => bold ? (
        <b><span className="white">{children}</span></b>
    ) : (
        <span className="white">{children}</span>
    );

export default Purple;