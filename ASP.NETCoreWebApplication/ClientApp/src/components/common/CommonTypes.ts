/**
 * Anything you want to display on screen, usually JSX or string
 */
type ArbitraryOutputValue = JSX.Element | JSX.Element[] | String | Number;

export interface ColoredElementProps {
    children: ArbitraryOutputValue;
    bold?: Boolean;
}

export interface BlockProps {
    isFlex?: boolean;
    height?: Number | String;
    collapse?: boolean;
    shrinkToContent?: boolean;
    padding?: Number | String;
    children?: ArbitraryOutputValue;
    
}