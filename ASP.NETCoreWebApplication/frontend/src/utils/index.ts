export * from "./route-paths";

export function buildQuery(query: Record<string, string>): string {
    return Object.keys(query)
        .reduce((prevStr: string, param: string) => {
            return prevStr + param + "=" + query[param] + "&";
        }, "")
        .slice(0, -1);
}

export function capitalize(input: string): string {
    const [first, ...rest] = input.split("");
    if (input === "") return "";
    if (input.length === 1) return first.toUpperCase();
    return first.toUpperCase() + rest.map((r: string) => r.toLowerCase()).join("");
}

export async function navigateTo(src: string) {
    window.open(src, "_blank");
}
