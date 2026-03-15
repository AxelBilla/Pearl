import postgres from 'postgres'
import crypto from 'crypto';

const sql = postgres({
    host                 : 'localhost',            // Postgres ip address[s] or domain name[s]
    port                 : 5432,          // Postgres server port[s]
    database             : '',            // Name of database to connect to
    username             : '',            // Username of database user
    password             : '',            // Password of database user
})
export class Database{
    static Get = class{
        static async Table(table, order_by, limit=20){
            if(limit!=-1) {
                return await sql`
                    SELECT *
                    FROM ${sql(table)}
                    ORDER BY ${sql(order_by)} DESC
                `
            } else {
                return await sql`
                    SELECT * FROM ${sql(table)}
                    ORDER BY ${sql(order_by)} DESC
                `
            }
        }

        static async Access(auth){
            console.log(auth);
            let res = await sql`
                SELECT EXISTS(
                    SELECT username FROM users
                    WHERE username = ${auth.username} AND password = ${auth.password}
                )
                `
            return res[0].exists;
        }

        static async ID(auth){
            let res = await sql`    
                SELECT id FROM users
                WHERE username = ${auth.username} AND password = ${auth.password}
                `
            return res[0].id;
        }
        static async Name(id){
            let res = await sql`    
                SELECT username FROM users
                WHERE id = ${id}
                `
            return res[0].username;
        }
        static async Admin(id){
            let res = await sql`
                SELECT EXISTS(
                    SELECT id FROM admins
                    WHERE id = ${id}
                )
                `
            return res[0].exists;

        }

    }
    static Delete = class{
        static async Entry(table, owner, pair, is_admin){
            if(is_admin){
                await sql`
                    DELETE  FROM ${sql(table)}
                    WHERE ${sql(pair.property)} = ${pair.value}
                `
            }
            else{
                await sql`
                    DELETE  FROM ${sql(table)}
                    WHERE ${sql(pair.property)} = ${pair.value}
                    AND ${sql(owner.property)} = ${owner.value}
                `
            }
        }

    }
    static Update = class{
        static async Entry(table, owner, condition, data, is_admin){
            console.log(data);
            if(is_admin){
                await sql`
                    UPDATE ${sql(table)}
                    SET ${sql(data)}
                    WHERE ${sql(condition.property)} = ${condition.value}
                `
            }
            else{
                await sql`
                    UPDATE ${sql(table)}
                    SET ${sql(data)}
                    WHERE ${sql(condition.property)} = ${condition.value}
                    AND ${sql(owner.property)} = ${owner.value}
                `
            }
        }

    }
    static Create = class{
        static async Entry(table, data){

            data.id = ID.Generate();

            await sql`
                    INSERT INTO ${sql(table)} ${sql(data)}
                `
        }

    }
}

export class Pair{
    constructor(property, value) {
        this.property = property;
        this.value = value;
    }
}

class ID{
    static Generate(){
        return crypto.randomUUID();
    }
}