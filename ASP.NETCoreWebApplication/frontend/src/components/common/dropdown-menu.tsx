import * as React from "react";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import MenuItem from "@mui/material/MenuItem";
import Menu from "@mui/material/Menu";

interface IDropdownMenuProps {
    options: string[];
    currentValue: string;
    displayName: string[];
    outputCallback: (value: string) => void;
}

export function DropdownMenu({ options, displayName, currentValue, outputCallback }: IDropdownMenuProps) {
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const [selectedIndex, setSelectedIndex] = React.useState(options.indexOf(currentValue));
    const open = Boolean(anchorEl);
    const handleClickListItem = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleMenuItemClick = (event: React.MouseEvent<HTMLElement>, index: number) => {
        setSelectedIndex(index);
        setAnchorEl(null);
        outputCallback(options[index]);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <div>
            <List component="div" aria-label="Device settings" sx={{ bgcolor: "background.paper" }}>
                <ListItem button id="lock-button" onClick={handleClickListItem}>
                    <ListItemText primary="Property type" secondary={displayName[selectedIndex]} />
                </ListItem>
            </List>
            <Menu
                id="lock-menu"
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
                MenuListProps={{
                    role: "listbox"
                }}
            >
                {options.map((option, index) => (
                    <MenuItem
                        key={option}
                        selected={index === selectedIndex}
                        onClick={event => handleMenuItemClick(event, index)}
                    >
                        {displayName ? displayName[index] : option}
                    </MenuItem>
                ))}
            </Menu>
        </div>
    );
}
