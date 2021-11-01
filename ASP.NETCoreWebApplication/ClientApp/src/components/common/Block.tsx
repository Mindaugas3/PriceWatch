import React from "react";
import { BlockProps } from "./CommonTypes";

const Block = (props: BlockProps) => {
    const { isFlex, collapse, shrinkToContent, padding } = props;
    return (
        <div style={
            {width: shrinkToContent ? "max-content" : "100%", 
            height: props.height || "max-content", 
            backgroundColor: props.bgcolor || "rgb(228, 47, 228)",
            minHeight: !isFlex && !collapse && "60vh",
            display: isFlex ? "flex" : "block",
            flexDirection: isFlex && "row",
            flexWrap: isFlex && "wrap",
            paddingTop: padding || 0,
            paddingBottom: padding || 0
        }}>{props.children}</div>
    )
}

export default Block;