import React, { ReactNode } from "react";
// import "./PwCss.css";
import { ColoredElementProps } from "./CommonTypes";


const Purple: (props: ColoredElementProps & { children: ReactNode }) => JSX.Element = ({children, bold = false}) => bold ? (
        <b><span className="purple">{children}</span></b>
    ) : (
        <span className="purple">{children}</span>
    );

export default Purple;