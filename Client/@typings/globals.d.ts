export { };

declare global {
    interface IBlogUser {
        /** The unique, numeric user ID */
        id: number;

        /** The display name */
        username: string;

        /** The users real name */
        name: string;

        /** The users email address */
        email: string;

        /** The users registration date */
        createDate: Date;

        /** Is the user an admin? */
        isAdmin: boolean;

        isAnonymous: boolean;
    }

    interface ISiteConfig {
        /** The root of the site */
        backendUrl: string;
    }

    var currentUser: IBlogUser;
    var siteConfig: ISiteConfig;
}
