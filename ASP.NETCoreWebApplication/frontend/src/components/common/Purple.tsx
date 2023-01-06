import React, { ReactNode } from "react";
import { ColoredElementProps } from "./common-types";


const Purple: (props: ColoredElementProps & { children: ReactNode }) => JSX.Element = ({children, bold = false}) => bold ? (
        <b><span className="purple">{children}</span></b>
    ) : (
        <span className="purple">{children}</span>
    );

export default Purple;